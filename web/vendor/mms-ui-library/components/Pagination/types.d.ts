export interface PaginationProps {
    total: number;
    perPage: number;
    perPageOptions: number[];
    onPerPageChange: (perPage: number) => void;
    currentPage: number;
    onCurrentPageChange: (currentPage: number) => void;
    dataTestId?: string;
}
