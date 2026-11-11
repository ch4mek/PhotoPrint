import { WorkLoad } from '@/pages/Payload/types';

interface WorkloadsOptions {
	isSubRows?: boolean;
	needBackground?: boolean;
	newHeight?: number;
	isExpanded?: boolean;
}

export interface WorkloadCellProps {
	projectId?: number;
	disabled: boolean;
	workloads: WorkLoad;
	options?: WorkloadsOptions;
	handleRowChange?: (newPlannedHour: number) => void;
	userId?: number;
	month?: number;
	year?: number;
	hasActivity?: boolean;
	isLastRow?: boolean;
}
export interface UpperDateCellAttrs {
	backgroundColor: string;
	isDisabled: boolean;
	isClickable: boolean;
	isSubRows?: boolean;
	minHeight?: number;
}
export interface WorkloadCellWrapperAttrs {
	maxHeight?: number;
}

export interface LowerDateCellAttrs {
	height?: number;
	disabled?: boolean;
}
