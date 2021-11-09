namespace WorkoutNoteApi

open Falco 
open FSharp.Json

module Handler = 
    let private ofJson a = a |> Json.serialize |> Response.ofPlainText

    let private handleError = Fail >> ofJson

    let getWorkoutByDate : HttpHandler = 
        let getWorkout (r : RouteCollectionReader) = 
            match r.TryGetDateTime "date" with 
            | Some d -> d |> Repository.getWorkoutByDate |> Json.serialize
            | None -> Fail "invalid date" |> Json.serialize
        Request.mapRoute getWorkout Response.ofPlainText

    let addWorkout : HttpHandler =
        let handleOk = Repository.addWorkout >> ofJson
        Request.bindJson handleOk handleError

    let addExercise : HttpHandler = 
        let handleOk = Repository.addExercise >> ofJson
        Request.bindJson handleOk handleError

    let updateExercise : HttpHandler = 
        let handleOk = Repository.updateExercise >> ofJson
        Request.bindJson handleOk handleError

    let deleteExercise : HttpHandler = 
        let getId (r : RouteCollectionReader) = 
            match r.TryGetInt "id" with 
            | Some id -> id |> Repository.deleteExercise |> Json.serialize
            | None -> Fail "invalid id" |> Json.serialize
        Request.mapRoute getId Response.ofPlainText

    let getSetsByExerciseId : HttpHandler = 
        let getSets (r : RouteCollectionReader) = 
            match r.TryGetInt "exercise_id" with 
            | Some e -> e |> Repository.getSetsByExerciseId |> Json.serialize
            | None -> Fail "invalid date" |> Json.serialize
        Request.mapRoute getSets Response.ofPlainText   

    let addSet : HttpHandler = 
        let handleOk = Repository.addSet >> ofJson
        Request.bindJson handleOk handleError

    let updateSet : HttpHandler = 
        let handleOk = Repository.updateSet >> ofJson
        Request.bindJson handleOk handleError

    let deleteSet : HttpHandler = 
        let getId (r : RouteCollectionReader) = 
            match r.TryGetInt "id" with 
            | Some id -> id |> Repository.deleteExercise |> Json.serialize
            | None -> Fail "invalid id" |> Json.serialize
        Request.mapRoute getId Response.ofPlainText                 