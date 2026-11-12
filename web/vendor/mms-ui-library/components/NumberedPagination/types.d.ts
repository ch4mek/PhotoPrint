import type { Theme } from '@/types/theme';
export interface NumberedPaginationProps {
    total: number;
    perPage: number;
    perPageOptions: number[];
    onPerPageChange: (perPage: number) => void;
    currentPage: number;
    onCurrentPageChange: (currentPage: number) => void;
    dataTestId?: string;
}
type PageType = 'default' | 'current';
export interface PageContainerProps {
    theme?: Theme;
    pageType: PageType;
    dots: boolean;
    dataTestId?: string;
}
export {};
