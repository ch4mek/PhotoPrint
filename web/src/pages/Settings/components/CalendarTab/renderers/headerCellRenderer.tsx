import { RenderHeaderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';
import { WorkingHoursCalendarRow } from '@/types/settings';

import { CellWrapper } from './styles';

export const hoursHeaderCellRenderer = ({
	column: { name },
}: RenderHeaderCellPropsWithHoverIndex<WorkingHoursCalendarRow, unknown>) => (
	<CellWrapper>{name}</CellWrapper>
);
