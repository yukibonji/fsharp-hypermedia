module HalSharp

open System
open Chiron

module Types =

    type Link = {
        href: string
        templated: bool option
        mediaType: string option
        deprication: Uri option
        name: string option
        profile: Uri option
        title: string option
        hreflang: string option
    }

    type Resource = {
        links: Map<string, Link>
        embedded: Embedded
        properties: Map<string, Json>
    }
    and Embedded = Map<string, Resource>

module Links =
    open Types
    let simpleLink href = {
        href = href
        templated = None
        mediaType = None
        deprication = None
        name = None
        profile = None
        title = None
        hreflang = None 
    }

    let serializeLink link : Json = 
        Object <| Map.ofList
            [ yield ("href", String link.href)
              yield! match link.templated with Some b -> [ ("templated", Bool b) ] | _ -> [] ]

module Resources =
    open Types
    open Links

    let serializeResource resource : Json =
        let createLink (rel, link) = rel, serializeLink link

        resource.links
        |> Map.toList
        |> List.map createLink
        |> Map.ofList |> Object
        |> fun links -> Map.ofList [ "_links", links ] |> Object