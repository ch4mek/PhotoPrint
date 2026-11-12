import { Dispatch, SetStateAction, ReactNode } from 'react';
export declare const FilterContext: import("react").Context<{
    filterId: string;
    setFilterId: Dispatch<SetStateAction<string>>;
} | null>;
export declare function FilterContextProvider({ children }: {
    children: ReactNode;
}): import("react/jsx-runtime").JSX.Element;
