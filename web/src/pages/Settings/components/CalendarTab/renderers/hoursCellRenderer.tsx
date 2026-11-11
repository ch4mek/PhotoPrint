import { HoursInputCell } from '../components/HoursInputCell';
import { DEFAULT_MONTH_TIME } from '../constants';

import { isChiefRole, isHeadOrGroupManager } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { skeletonRenderer } from '@/pages/Settings/components/CalendarTab/renderers/skeletonRenderer';
import { RenderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';
import {
	WorkingHoursCalendarRow,
	WorkingHoursCalendarRowType,
} from '@/types/settings';

import { CellWrapper } from './styles';

export const hoursCellRenderer = (
	{
		row,
		rowIdx,
	}: RenderCellPropsWithHoverIndex<WorkingHoursCalendarRow, unknown>,
	monthId: number,
	isLoading: boolean
) => {
	const { type, timeMap, countryId, disabled } = row;

	const { role } = useAuth();
	const shouldDisplayTooltip =
		!isHeadOrGroupManager(role) &&
		type === WorkingHoursCalendarRowType.NewCountryRow;

	if (type === WorkingHoursCalendarRowType.AddCountryButtonRow) {
		return <CellWrapper />;
	}

	if (isLoading) {
		return skeletonRenderer();
	}

	if (isLoading) {
		return skeletonRenderer();
	}

	const {
		id,
		hours,
		isLoading: isTimeMapLoading,
		hasError,
	} = timeMap.get(monthId) || DEFAULT_MONTH_TIME;

	const isDisableInputCell = disabled || isTimeMapLoading || !isChiefRole(role);

	return (
		<CellWrapper>
			<HoursInputCell
				countryId={countryId}
				id={id}
				time={hours ? String(hours) : null}
				rowIdx={rowIdx}
				monthId={monthId}
				disabled={isDisableInputCell}
				shouldDisplayTooltip={shouldDisplayTooltip}
				hasError={hasError}
			/>
		</CellWrapper>
	);
};
