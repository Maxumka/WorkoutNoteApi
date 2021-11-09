open Falco.Routing
open Falco.HostBuilder
open WorkoutNoteApi

webHost [||] {
    endpoints [
        // query for workout
        get "/getWorkoutByDate/{date}" Handler.getWorkoutByDate
        post "/addWorkout" Handler.addWorkout

        // query for exercise
        get "/getSetsByExerciseId/{exercise_id}" Handler.getSetsByExerciseId     
        post "/addExercise" Handler.addExercise
        put "/updateExercise" Handler.updateExercise
        delete "/deleteExercise/{id}" Handler.deleteExercise

        // query for set
        post "/addSet" Handler.addSet
        put "/updateSet" Handler.updateSet
        delete "/deleteSet" Handler.deleteSet
    ]
}
