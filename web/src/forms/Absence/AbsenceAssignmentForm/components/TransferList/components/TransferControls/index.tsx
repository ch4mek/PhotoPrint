import { ChevronLeftIcon } from '@mms/mms-ui-library/icons/base/ChevronLeftIcon';
import { ChevronRightIcon } from '@mms/mms-ui-library/icons/base/ChevronRightIcon';
import { DoubleChevronLeftIcon } from '@mms/mms-ui-library/icons/base/DoubleChevronLeftIcon';
import { DoubleChevronRightIcon } from '@mms/mms-ui-library/icons/base/DoubleChevronRightIcon';

import { ControlButton } from '../ControlButton';

import { transferControlsGridAreas } from './constants';
import { TransferControlsWrapper } from './styles';
import { TransferControlsProps } from './types';

const { allRight, checkedRight, allLeft, checkedLeft } =
	transferControlsGridAreas;

export function TransferControls({
	handleAllRight,
	handleCheckedRight,
	handleCheckedLeft,
	handleAllLeft,
	isAllRightDisabled,
	isCheckedRight,
	isAllLeft,
	isCheckedLeft,
}: TransferControlsProps) {
	return (
		<TransferControlsWrapper>
			<ControlButton
				icon={<DoubleChevronRightIcon />}
				onClick={handleAllRight}
				disabled={isAllRightDisabled}
				gridArea={allRight}
			/>
			<ControlButton
				icon={<ChevronRightIcon />}
				onClick={handleCheckedRight}
				disabled={isCheckedRight}
				gridArea={checkedRight}
			/>
			<ControlButton
				icon={<ChevronLeftIcon />}
				onClick={handleCheckedLeft}
				disabled={isCheckedLeft}
				gridArea={checkedLeft}
			/>
			<ControlButton
				icon={<DoubleChevronLeftIcon />}
				onClick={handleAllLeft}
				disabled={isAllLeft}
				gridArea={allLeft}
			/>
		</TransferControlsWrapper>
	);
}
