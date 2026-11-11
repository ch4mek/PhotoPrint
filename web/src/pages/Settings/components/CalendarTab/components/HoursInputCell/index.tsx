import React, { MouseEvent, useMemo } from 'react';

import { useCalendarContextOperations } from '../../hooks/useCalendarOperations';

import {
	NOT_VALID_COUNTRY_HOURS_RANGE,
	COUNTRY_FIRST,
} from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { useToast } from '@/context/index';
import { usePrevValueKeeperContext } from '@/context/PrevStringValueKeeper';
import { HoursInput } from '@/fields/Time/HoursInput';
import { createToast } from '@/helpers/createToast';

import { hoursSchema } from './schema';
import { InputContainer } from './styles';
import { InputTimeCellProps } from './types';

const DEFAULT_INPUT_VALUE = '';

function HoursInputCellInner({
	time,
	rowIdx,
	monthId,
	id,
	countryId,
	disabled,
	shouldDisplayTooltip,
	hasError,
}: InputTimeCellProps) {
	const {
		year,
		onMonthTimeChange,
		createMonthTime,
		updateMonthTime,
		setNewMonthTime,
		setIsMonthTimeLoading,
		handleCellClick,
		handleCellKeyDown,
		handleInputKeyDown,
		setTooltipContent,
		setCellHasError,
	} = useCalendarContextOperations();

	const prevValueKeeper = usePrevValueKeeperContext();

	const toast = useToast();

	const isDisabled = disabled || countryId === 0;

	const handleChangeInput = (value: string) => {
		onMonthTimeChange(rowIdx, monthId, value ? Number(value) : 0);
	};

	const handleInputBlur = () => {
		const prevMonthTime = prevValueKeeper.get();

		if (prevMonthTime !== null && time !== null && prevMonthTime === time) {
			return;
		}

		try {
			hoursSchema.validateSync(time);
			setCellHasError(rowIdx, monthId, false);
		} catch {
			setCellHasError(rowIdx, monthId, true);
			toast.open(
				createToast(NotificationType.ERROR, NOT_VALID_COUNTRY_HOURS_RANGE)
			);

			return;
		}

		setIsMonthTimeLoading(rowIdx, monthId, true);

		if (id) {
			updateMonthTime(
				id,
				time ? Number(time) : 0,
				() => {
					setIsMonthTimeLoading(rowIdx, monthId, false);
				},
				() => {
					setIsMonthTimeLoading(rowIdx, monthId, false);
				}
			);

			return;
		}

		createMonthTime(
			{
				countryId,
				hours: time ? Number(time) : 0,
				monthOfYear: monthId,
				year,
			},
			({ hours, id: monthTimeId }) => {
				setNewMonthTime(rowIdx, monthId, {
					hours,
					id: monthTimeId,
					isLoading: false,
					hasError: false,
				});
			},
			() => {
				setIsMonthTimeLoading(rowIdx, monthId, false);
			}
		);
	};
	const handleInputFocus = () => {
		prevValueKeeper.set(time);
	};

	const handleMouseOver = useMemo(() => {
		if (shouldDisplayTooltip) {
			return (e: MouseEvent<HTMLDivElement>) => {
				setTooltipContent({
					text: COUNTRY_FIRST,
					anchorElement: e.currentTarget,
				});
			};
		}

		return undefined;
	}, [shouldDisplayTooltip]);

	const handleMouseOut = useMemo(() => {
		if (shouldDisplayTooltip) {
			return () => {
				setTooltipContent(null);
			};
		}

		return undefined;
	}, [shouldDisplayTooltip]);

	return (
		<InputContainer
			onClick={handleCellClick}
			onKeyDown={handleCellKeyDown}
			onMouseOver={handleMouseOver}
			onMouseOut={handleMouseOut}
		>
			<HoursInput
				value={time === null ? DEFAULT_INPUT_VALUE : time}
				onChange={handleChangeInput}
				disabled={isDisabled}
				onBlurEvent={handleInputBlur}
				onFocusEvent={handleInputFocus}
				onKeyDown={handleInputKeyDown}
				hasError={hasError}
			/>
		</InputContainer>
	);
}

export const HoursInputCell = React.memo(HoursInputCellInner);
