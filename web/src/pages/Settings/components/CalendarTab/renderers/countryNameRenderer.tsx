import { AddButtonCell } from '../components/AddButtonCell';
import { CountryNameCell } from '../components/CountryNameCell';
import { ADD_BUTTON_CELL_CLASS_NAME } from '../constants';
import { useCalendarContextOperations } from '../hooks/useCalendarOperations';

import { skeletonRenderer } from '@/pages/Settings/components/CalendarTab/renderers/skeletonRenderer';
import { RenderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';
import {
	WorkingHoursCalendarRow,
	WorkingHoursCalendarRowType,
} from '@/types/settings';

import { CellWrapper } from './styles';

export const countryNameRenderer = (
	{
		row,
		rowIdx,
	}: RenderCellPropsWithHoverIndex<WorkingHoursCalendarRow, unknown>,
	isLoading: boolean
) => {
	const { type, error, countryName, countryId, disabled } = row;
	const { onAddNewRow } = useCalendarContextOperations();

	if (type === WorkingHoursCalendarRowType.AddCountryButtonRow) {
		const onAddClick = (callback: () => void) => {
			onAddNewRow(rowIdx - 1, row, callback);
		};

		return (
			<CellWrapper fullHeight={false} className={ADD_BUTTON_CELL_CLASS_NAME}>
				<AddButtonCell
					disabled={disabled || Boolean(error) || countryName === ''}
					onAddClick={onAddClick}
				/>
			</CellWrapper>
		);
	}

	if (isLoading) {
		return skeletonRenderer();
	}

	return (
		<CellWrapper>
			<CountryNameCell
				countryId={countryId}
				countryName={countryName}
				error={error}
				rowIdx={rowIdx}
			/>
		</CellWrapper>
	);
};
