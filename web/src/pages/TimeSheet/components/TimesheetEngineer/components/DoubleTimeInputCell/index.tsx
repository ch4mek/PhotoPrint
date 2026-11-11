import { useContext, useEffect, useMemo, useRef, useState } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { TimeSheetContext } from '../../..';
import { TimeSheetRow } from '../../types';
import { InputWrapper } from '../CellInput/styles';

import { minutesToHours } from '@/helpers/minutesToHours';
import { getTimeFromTrackItem } from '@/pages/TimeSheet/components/TimesheetEngineer/components/DoubleTimeInputCell/utils/getTimeFromTrackItem';
import { useTooltipHandler } from '@/pages/TimeSheet/components/TimesheetEngineer/hooks/useTooltipHandler';
import { TrackedTimeType } from '@/types/index';

import { EditableInputCell } from './components/EditableInputCell';
import { DoubleCellContentWrapper } from './styles';

const { Fact, Billable } = TrackedTimeType;

export function DoubleTimeInputCell(
	props: RenderCellProps<TimeSheetRow, unknown>
) {
	const [isActive, setIsActive] = useState<'top' | 'bottom' | null>(null);
	const factInputRef = useRef<HTMLInputElement>(null);
	const billableInputRef = useRef<HTMLInputElement>(null);
	const { column, row } = props;
	const {
		isDisabledTimesheet,
		isEngineer,
		isActiveUser,
		displayTooltips,
		isPMOEmployee,
		setTooltipContent,
	} = useContext(TimeSheetContext);

	const { key } = column;
	const { timeMap, areTimeCellsEditable, errorMessage } = row;

	const trackItem = timeMap.get(key);

	const { handleMouseOut, handleMouseOver, tooltipMessage } = useTooltipHandler(
		{
			row,
			column,
			isEngineer: isEngineer || isPMOEmployee,
			displayTooltips,
			setTooltipContent,
		}
	);

	const isCellDisabled = useMemo(
		() =>
			isDisabledTimesheet ||
			isActiveUser === false ||
			Boolean(tooltipMessage) ||
			Boolean(errorMessage) ||
			!areTimeCellsEditable,
		[
			isDisabledTimesheet,
			isActiveUser,
			tooltipMessage,
			errorMessage,
			areTimeCellsEditable,
		]
	);

	const isBillableDisabled = trackItem?.isLoading?.billable || isCellDisabled;
	const isFactDisabled = trackItem?.isLoading?.fact || isCellDisabled;

	const [factTimeInput, setFactTimeInput] = useState<string>(
		minutesToHours(getTimeFromTrackItem(trackItem, Fact))
	);
	const [billableTimeInput, setBillableTimeInput] = useState<string>(
		minutesToHours(getTimeFromTrackItem(trackItem, Billable))
	);

	useEffect(() => {
		switch (isActive) {
			case 'top':
				factInputRef.current?.focus();
				break;
			case 'bottom':
				billableInputRef.current?.focus();
				break;

			default:
		}
	}, [isActive]);

	return (
		<DoubleCellContentWrapper
			onMouseEnter={handleMouseOver}
			onMouseOut={handleMouseOut}
			data-cell="double-cell"
		>
			{isActive === null ? (
				<>
					<InputWrapper
						disabled={isFactDisabled}
						onClick={() => setIsActive('top')}
						cellPosition="top"
					>
						<input defaultValue={factTimeInput} disabled={isFactDisabled} />
					</InputWrapper>
					<InputWrapper
						disabled={isBillableDisabled}
						cellPosition="bottom"
						onClick={() => setIsActive('bottom')}
					>
						<input
							defaultValue={billableTimeInput}
							disabled={isBillableDisabled}
						/>
					</InputWrapper>
				</>
			) : (
				<EditableInputCell
					{...props}
					factInputRef={factInputRef}
					billableInputRef={billableInputRef}
					factTimeInput={factTimeInput}
					billableTimeInput={billableTimeInput}
					isBillableDisabled={isBillableDisabled}
					isFactDisabled={isFactDisabled}
					setFactTimeInput={setFactTimeInput}
					setBillableTimeInput={setBillableTimeInput}
					isCellDisabled={isCellDisabled}
				/>
			)}
		</DoubleCellContentWrapper>
	);
}
