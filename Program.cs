using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Task {
    public int ID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }

    public Task(int ID, string Title, string Description, DateTime DueDate, string Status) {
        this.ID = ID;
        this.Title = Title;
        this.Description = Description;
        this.DueDate = DueDate;
        this.Status = Status;
    }

    // public override string ToString() {
    //     return $"ID: {ID}, Title: {Title}, Description: {Description}, DueDate: {DueDate}, Status: {Status}";
    // }
}

public class TaskManager {
    private List<Task> task = new List<Task>();
    private static Random random = new Random();

    private const string FilePath = "tasks.json";

    public TaskManager() {
        LoadTask();
    }
    public void AddTask(string title, string description, DateTime dueTime) {
       int Id = random.Next(0, 9999);
       Task newTask = new Task(Id, title, description, dueTime, "Pending");
       task.Add(newTask);
       SaveTask();
    }

    public List<Task> GetAllTasks() => task;

    public Task? GetTaskById(int id) => task.Find(t => t.ID == id);

    public void UpdateTask(int id, string title, string description, DateTime dueTime, string status) {
        var task = GetTaskById(id);
        if (task != null) {
            task.Title = title;
            task.Description = description;
            task.DueDate = dueTime;
            task.Status = status;
            SaveTask();
        }
    }

    public void  DeleteTask(int id) {
        task.RemoveAll(t => t.ID == id);
        SaveTask();
    }

    public void MarkTaskAsCompleted(int id) {
        var task = GetTaskById(id);
        if (task != null) {
            task.Status = "Completed";
            SaveTask();
        }
    }

    private void SaveTask() {
       try {
        var json = JsonConvert.SerializeObject(task, Formatting.Indented);
        File.WriteAllText(FilePath, json);        
       } catch (Exception e) {
        Console.WriteLine($"An error occurred while saving tasks: {e.Message}");
       }
    }

    private void LoadTask() {
       try {
         if(File.Exists(FilePath)) {
            var json = File.ReadAllText(FilePath);
            task = JsonConvert.DeserializeObject<List<Task>>(json) ?? new List<Task>();
        }
       } catch (Exception e) {
        Console.WriteLine($"An error occurred while loading tasks: {e.Message}");

       }
    }
}


public class Program {
    static void Main(string[] args) {
        TaskManager taskManger = new TaskManager();
        while(true) {
            Console.WriteLine("Manage Your Tasks");
            Console.WriteLine("1.) Add Task");
            Console.WriteLine("2.) View Tasks");
            Console.WriteLine("3.) Update Task");
            Console.WriteLine("4.) Delete Task");
            Console.WriteLine("5.) Mark Task as Completed");
            Console.WriteLine("6.) Exit");
            Console.WriteLine("Choose an Option");

            if(!int.TryParse(Console.ReadLine(), out int choice)) {
                Console.WriteLine("Invalid Input, please enter a number.");
                continue;
            }

            switch(choice) {
                case 1:
                    Console.WriteLine("Enter A Description: ");
                    string? title = Console.ReadLine();

                    Console.WriteLine("Enter A Description: ");
                    string? description = Console.ReadLine();

                    Console.WriteLine("Enter Due Date (yyyy-mm-dd): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime dueDate)) {
                        Console.WriteLine("Invalid Date Format");
                    }

                    taskManger.AddTask(title ?? "", description ?? "", dueDate);
                break;

                case 2:
                    var tasks = taskManger.GetAllTasks();
                    if (tasks.Count < 1) {
                        Console.WriteLine("No Task");
                    } else {
                        foreach (var task in tasks) {
                            Console.WriteLine($"ID: {task.ID}, Title: {task.Title}, Description: {task.Description}, DueDate: {task.DueDate}, Status: {task.Status}");
                        }
                    }
                break;
                

                case 3:
                    Console.WriteLine("Enter Task Id TO Edit Task: ");
                    if (!int.TryParse(Console.ReadLine(), out int updateId)) {
                        Console.WriteLine("Invalid input, please enter a number.");
                        break;
                    }

                    var taskIdToUpdate = taskManger.GetTaskById(updateId);
                    if (taskIdToUpdate != null) {
                        Console.WriteLine("Edit Task Title: ");
                        taskIdToUpdate.Title = Console.ReadLine() ?? "";

                        Console.WriteLine("Edit Task Description: ");
                        taskIdToUpdate.Description = Console.ReadLine() ?? "";

                        Console.WriteLine("Edit Task Due Date (yyyy-mm-dd): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out DateTime updatedDueDate)) {
                            Console.WriteLine("Invalid date format.");
                            break;
                        }

                        Console.WriteLine("Edit Task Status: ");
                        taskIdToUpdate.Status = Console.ReadLine() ?? "";

                        taskManger.UpdateTask(updateId, taskIdToUpdate.Title, taskIdToUpdate.Description, updatedDueDate, taskIdToUpdate.Status);
                    } else {
                        Console.WriteLine("Invalid Task ID");
                    }
                break;

                case 4:
                    Console.WriteLine("Enter Task Id To Delete Task: ");
                    if (!int.TryParse(Console.ReadLine(), out int deleteId)) {
                        Console.WriteLine("Invalid input, please enter a number.");
                        break;
                    }
                    taskManger.DeleteTask(deleteId);
                    Console.WriteLine("Task Deleted");
                break;

                case 5:
                    Console.WriteLine("Enter Task Id To Mark Task as Completed: ");
                    if (!int.TryParse(Console.ReadLine(), out int completedId)) {
                        Console.WriteLine("Invalid input, please enter a number.");
                        break;
                    }
                    var completedTask = taskManger.GetTaskById(completedId);
                    if (completedTask != null) {
                        taskManger.MarkTaskAsCompleted(completedId);
                        Console.WriteLine("Task Updated to Completed");

                    } else {
                        Console.WriteLine("Task not Found");
                    }
                break;

                case 6:
                    return;

                default:
                    Console.WriteLine("Invalid Command. Enter a Number from 1 to 6");
                break;
            }
        }
    }
}

