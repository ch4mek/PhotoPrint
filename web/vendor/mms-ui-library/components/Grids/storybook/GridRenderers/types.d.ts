export interface GridRowDefaultProps {
    id?: string | number;
    showSkeleton?: boolean;
    handleEditClick?: (equipmentId?: string | number) => () => void;
    disabled?: boolean;
}
export interface ColorCircleGridRowDefaultProps extends GridRowDefaultProps {
    color: string;
}
export interface CellTextProps {
    disabled?: boolean;
    padding?: string | number;
}
