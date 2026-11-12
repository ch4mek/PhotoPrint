import type { OnCreateOption } from '../types';
export declare function useCreateService<T>(onCreateOption?: OnCreateOption<T>): {
    isCreating: boolean;
    handleCreate: (title?: string, existing?: T[]) => Promise<T[] | undefined>;
};
