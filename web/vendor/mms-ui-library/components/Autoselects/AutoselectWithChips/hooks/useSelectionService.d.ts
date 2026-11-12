import type { OnChange } from '../types';
import { GetOptionLabel } from '@/types/common';
export declare function useSelectionService<T>(onChange: OnChange<T>, getOptionLabel?: GetOptionLabel<T>, dropTabValueField?: string, setFocusAndScroll?: () => void, clearSearch?: (value: string) => void): {
    applySelection: (selected: T[] | T | null) => void;
};
