import { TotalHoursCell } from '../components/TotalHoursCell';

import { skeletonRenderer } from '@/pages/Settings/components/CalendarTab/renderers/skeletonRenderer';
import { HOVER_TOTAL_CELL_CLASS_NAME } from '@/pages/TimeSheet/components/TimeSheeetManager/constants';
import { RenderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';
import {
	WorkingHoursCalendarRow,
	WorkingHoursCalendarRowType,
} from '@/types/settings';

import { TotalCellWrapper } from './styles';

export function totalCellRenderer(
	props: RenderCellPropsWithHoverIndex<WorkingHoursCalendarRow, unknown>,
	isLoading: boolean
) {
	const {
		row: { type },
	} = props;

	if (type === WorkingHoursCalendarRowType.AddCountryButtonRow) {
		return <TotalCellWrapper />;
	}

	if (isLoading) {
		return skeletonRenderer();
	}

	return (
		<TotalCellWrapper className={HOVER_TOTAL_CELL_CLASS_NAME}>
			<TotalHoursCell {...props} />
		</TotalCellWrapper>
	);
}
