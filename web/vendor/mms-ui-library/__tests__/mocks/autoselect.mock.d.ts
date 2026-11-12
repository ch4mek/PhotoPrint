export interface Project {
    id: number;
    name: string;
    color: string;
    type: string;
    client: string;
}
export interface User {
    id: number;
    firstName: string;
    lastName: string;
    positionDescription: string;
    color: string;
    avatarFileId: null;
}
export interface Icon {
    id: number;
    name: string;
    icon: string;
}
export declare const PROJECT_LIST: Project[];
export declare const USER_LIST: User[];
export declare const ICONS_LIST: Icon[];
export declare const simpleItemsList: string[];
export declare const LONG_USERS_LIST: ({
    id: number;
    firstName: string;
    lastName: string;
    positionDescription: string;
    color: string;
    avatarFileId: null;
} | {
    id: number;
    firstName: string;
    lastName: string;
    positionDescription: string;
    color: string;
    avatarFileId: string;
} | {
    id: number;
    firstName: string;
    lastName: string;
    positionDescription: string;
    color: null;
    avatarFileId: null;
})[];
