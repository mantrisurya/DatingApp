import { Photo } from "./photo";

  export interface Member {
    id: number;
    userName: string;
    photoUrl: string;
    age: number;
    created: Date;
    lastActive: Date;
    knownAs: string;
    gender: string;
    introduction: string;
    lookingFor: string;
    interests: string;
    city: string;
    country: string;
    photos?: Photo[];
  }
