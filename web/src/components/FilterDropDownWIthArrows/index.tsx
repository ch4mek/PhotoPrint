import { ChevronLeftIcon } from '@mms/mms-ui-library/icons/base/ChevronLeftIcon';
import { ChevronRightIcon } from '@mms/mms-ui-library/icons/base/ChevronRightIcon';
import { useCallback } from 'react';

import { FilterDropdown } from '../FilterDropdown';

import { ArrowsWrapper, ArrowWrapper, FilterWrapper } from './styles';
import { FilterDropDownWithArrowsProps } from './types';

export function FilterDropDownWithArrows<T = string>({
	onLeftArrowClick,
	isLeftArrowDisabled = false,
	onRightArrowClick,
	isRightArrowDisabled = false,
	width = 'large',
	fillAvailableWidthOnMobile,
	...restProps
}: FilterDropDownWithArrowsProps<T>) {
	const handleLeftClick = useCallback(() => {
		if (!isLeftArrowDisabled) {
			onLeftArrowClick();
		}
	}, [isLeftArrowDisabled, onLeftArrowClick]);

	const handleRightClick = useCallback(() => {
		if (!isRightArrowDisabled) {
			onRightArrowClick();
		}
	}, [isRightArrowDisabled, onRightArrowClick]);

	return (
		<FilterWrapper {...{ width, fillAvailableWidthOnMobile }}>
			<FilterDropdown
				{...restProps}
				width="fullWidth"
				stylesConfig={{
					width: 'auto',
				}}
				preventDropdownFlip
			/>
			<ArrowsWrapper>
				<ArrowWrapper disabled={isLeftArrowDisabled} onClick={handleLeftClick}>
					<ChevronLeftIcon />
				</ArrowWrapper>
				<ArrowWrapper
					disabled={isRightArrowDisabled}
					onClick={handleRightClick}
				>
					<ChevronRightIcon />
				</ArrowWrapper>
			</ArrowsWrapper>
		</FilterWrapper>
	);
}
