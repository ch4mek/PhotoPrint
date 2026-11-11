import { GridCellDefaultProps } from '@/components/Grid';
import { AbsenceStatusEntity } from '@/types/absences';
import { HandleRowClick } from '@/types/grid';

export interface StatusCellProps extends GridCellDefaultProps {
	status: AbsenceStatusEntity;
}

export interface RequestStatusCellRendererOptions<T> {
	onCellClick?: HandleRowClick<T>;
	isClickable?: boolean;
}
