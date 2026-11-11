import { useMemo } from 'react';

import { FilterDropDownWithArrows } from '@/components/FilterDropDownWIthArrows';
import { ResetFilterButton } from '@/components/ResetFilterButton';
import { getFilterYears } from '@/helpers/getFilterYears';

import { YearFilterControlsWrapper } from './styles';
import { YearFilterControlsProps } from './types';

export function YearFilterControls({
	year,
	onYearChange,
	isResetDisabled,
	onReset,
}: YearFilterControlsProps) {
	const yearsOptions = useMemo(() => getFilterYears(), []);

	const getNextYear = (step: 1 | -1) => {
		onYearChange((prevYear: string) => {
			const index = yearsOptions.indexOf(prevYear);
			const newIndex = index + step;

			if (index === -1 || newIndex < 0 || newIndex >= yearsOptions.length) {
				return prevYear;
			}

			return yearsOptions[newIndex];
		});
	};

	const handleLeftClick = () => {
		getNextYear(1);
	};

	const handleRightClick = () => {
		getNextYear(-1);
	};

	return (
		<YearFilterControlsWrapper>
			<FilterDropDownWithArrows
				width="small"
				onChange={onYearChange}
				value={year}
				options={yearsOptions}
				onLeftArrowClick={handleLeftClick}
				onRightArrowClick={handleRightClick}
				isLeftArrowDisabled={year === yearsOptions[yearsOptions.length - 1]}
				isRightArrowDisabled={year === yearsOptions[0]}
			/>
			<ResetFilterButton disabled={isResetDisabled} onClick={onReset} />
		</YearFilterControlsWrapper>
	);
}
