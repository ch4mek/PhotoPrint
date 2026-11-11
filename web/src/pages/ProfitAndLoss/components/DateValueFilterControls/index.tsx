import { DateValueFilter } from '@/components/DateValueFilter';
import { ResetFilterButton } from '@/components/ResetFilterButton';

import {
	DateValueFilterWrapper,
	DateValueFilterControlsWrapper,
} from './styles';
import { DateValueFilterControlsProps } from './types';

export function DateValueFilterControls({
	date,
	onDateChange,
	isResetDisabled,
	onReset,
}: DateValueFilterControlsProps) {
	return (
		<DateValueFilterControlsWrapper>
			<DateValueFilterWrapper>
				<DateValueFilter {...{ date, onDateChange }} />
			</DateValueFilterWrapper>
			<ResetFilterButton disabled={isResetDisabled} onClick={onReset} />
		</DateValueFilterControlsWrapper>
	);
}
