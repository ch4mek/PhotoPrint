import { useTheme } from '@emotion/react';

import { FilterAutoselect } from '@/components/FilterAutoselect';
import { FilterDropDownWithArrows } from '@/components/FilterDropDownWIthArrows';
import { ResetFilterButton } from '@/components/ResetFilterButton';

import { initialInquiryFilterValue } from './constants';
import { useCalendarFiltersController } from './hooks/useCalendarFiltersController';
import {
	AutoselectWrapper,
	ButtonWithFilterWrapper,
	CalendarFiltersWrapper,
} from './styles';

export function CalendarFilters() {
	const {
		inquiryOptions,
		selectedInquiryOption,
		handleInquiryChange,
		isResetDisabled,
		resetFilters,
		dateOptions,
		isNextArrowDisabled,
		isPrevArrowDisabled,
		handleDateChange,
		handleNextMonth,
		handlePrevMonth,
		selectedDateOption,
	} = useCalendarFiltersController();
	const theme = useTheme();

	return (
		<CalendarFiltersWrapper>
			<FilterDropDownWithArrows
				onChange={handleDateChange}
				value={selectedDateOption}
				options={dateOptions}
				onLeftArrowClick={handlePrevMonth}
				onRightArrowClick={handleNextMonth}
				isLeftArrowDisabled={isPrevArrowDisabled}
				isRightArrowDisabled={isNextArrowDisabled}
			/>
			<ButtonWithFilterWrapper>
				<AutoselectWrapper>
					<FilterAutoselect
						label=""
						valueField="name"
						value={selectedInquiryOption}
						valueToClean={initialInquiryFilterValue}
						options={inquiryOptions}
						onChange={handleInquiryChange}
						stylesConfig={{
							backgroundColor: theme.palette['background-white'],
						}}
					/>
				</AutoselectWrapper>
				<ResetFilterButton disabled={isResetDisabled} onClick={resetFilters} />
			</ButtonWithFilterWrapper>
		</CalendarFiltersWrapper>
	);
}
