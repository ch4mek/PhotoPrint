export type GetKey<T> = (option: T) => string | number;
export type GetOptionLabel<T> = (option: T) => {
    [key: string]: string;
};
export type CheckIfOptionEqualToValue<T> = (option: T, value: T) => boolean;
