namespace WorkoutNoteApi

open Npgsql.FSharp

module Repository = 
    let private dbConnect = 
        Sql.host "ec2-3-229-127-203.compute-1.amazonaws.com"
        |> Sql.port 5432
        |> Sql.database "d3anj3rcbua4r2"
        |> Sql.username "qdublpttcdvcri"
        |> Sql.password "d6900236d1cd37f5e22cd007da11403972a415ca68668df69507409e4c08585d"
        |> Sql.sslMode SslMode.Require
        |> Sql.trustServerCertificate true
        |> Sql.formatConnectionString
        |> Sql.connect
 
    let private getWorkoutByDate' date = 
        try
            dbConnect
            |> Sql.query "select * from Workout where date = @Date"
            |> Sql.parameters ["Date", SqlValue.Date date]
            |> Sql.executeRow (fun r -> 
            {
                Id = r.int "w_id"
                Date = r.dateTime "date"
                Exercises = []
            }) |> Ok 
        with  
        | ex -> Fail ex.Message         

    let getWorkoutByDate date = 
        try 
            dbConnect 
            |> Sql.query @"select w.*, e.* 
                           from Workout as w join Exercise as e on e.workout_id = w.w_id 
                           where w.date = @Date"
            |> Sql.parameters ["Date", SqlValue.Date date]
            |> Sql.execute (fun r -> 
            {
                Id = r.int "e_id"
                Name = r.string "name"
                Sets = []
                Workout = 
                    {
                        Id = r.int "w_id"
                        Date = date
                        Exercises = []
                    }
            })
            |> function 
                | [] -> getWorkoutByDate' date
                | (x::xs) -> {Id = x.Workout.Id; Date = date; Exercises = (x::xs)} |> Ok  
        with 
        | ex -> Fail $"Get workout with exericse: {ex.Message}"

    let addWorkout (workout : Workout) = 
        try 
            dbConnect 
            |> Sql.query "insert into Workout (date) values(@Date) returning w_id"
            |> Sql.parameters ["Date", SqlValue.Date workout.Date]
            |> Sql.executeRow (fun r -> r.int "w_id")
            |> Ok 
        with 
        | ex -> Fail ex.Message

    let addExercise (exercise : Exercise) = 
        try 
            dbConnect 
            |> Sql.query "insert into Exercise (name, workout_id) values(@Name, @Workout_id) returning e_id"
            |> Sql.parameters ["Name", Sql.string exercise.Name; "Workout_id", Sql.int exercise.Workout.Id]
            |> Sql.executeRow (fun r -> r.int "e_id")
            |> Ok
        with 
        | ex -> Fail ex.Message

    let updateExercise (exercise : Exercise) = 
        try 
            dbConnect
            |> Sql.query "update Exercise set name = @Name where e_id = @E_id"
            |> Sql.parameters ["Name", Sql.string exercise.Name; "E_id", Sql.int exercise.Id]
            |> Sql.executeNonQuery
            |> Ok
        with 
        | ex -> Fail ex.Message

    let deleteExercise id = 
        try 
            dbConnect
            |> Sql.query "delete from exercise where e_id = @E_id"
            |> Sql.parameters ["E_id", Sql.int id]
            |> Sql.executeNonQuery
            |> Ok 
        with 
        | ex -> Fail ex.Message

    let getSetsByExerciseId exerciseId = 
        try 
            dbConnect
            |> Sql.query "select * from Set where exercise_id = @Exercise_id"
            |> Sql.parameters ["Exercise_id", Sql.int exerciseId]
            |> Sql.execute (fun r -> 
            {
                Id = r.int "id"
                Weight = r.double "weight"
                Reps = r.int "reps"
                ExerciseId = exerciseId
            })  
            |> Ok
        with 
        | ex -> Fail ex.Message     

    let addSet (set : Set) = 
        try 
            dbConnect
            |> Sql.query "insert into Set (weight, reps, exercise_id) values (@Weight, @Reps, @Exercise_id) returning s_id"
            |> Sql.parameters ["Weight", Sql.double set.Weight; "Reps", Sql.int set.Reps; "Exercise_id", Sql.int set.ExerciseId]
            |> Sql.executeRow (fun r -> r.int "s_id")
            |> Ok 
        with 
        | ex -> Fail ex.Message

    let updateSet (set : Set) = 
        try 
            dbConnect 
            |> Sql.query "update Set SET weight = @Weight, reps = @Reps where s_id = @S_id"
            |> Sql.parameters ["Weight", Sql.double set.Weight; "Reps", Sql.int set.Reps; "S_id", Sql.int set.Id]    
            |> Sql.executeNonQuery
            |> Ok
        with 
        | ex -> Fail ex.Message

    let deleteSet id = 
        try 
            dbConnect
            |> Sql.query "delete from set where s_id = @S_id"
            |> Sql.parameters ["S_id", Sql.int id]
            |> Sql.executeNonQuery 
            |> Ok 
        with 
        | ex -> Fail ex.Message                   