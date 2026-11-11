import { memo } from 'react';

import { useTimelineContext } from '../TimelineContext';

import { HourCellWrapper } from './styles';

function HoursCellInner({ isLast }: { isLast: boolean }) {
	const { isPeriodSelectionDisabled } = useTimelineContext();

	return (
		<HourCellWrapper isLast={isLast} isDisabled={isPeriodSelectionDisabled} />
	);
}

export const HoursCell = memo(HoursCellInner);
