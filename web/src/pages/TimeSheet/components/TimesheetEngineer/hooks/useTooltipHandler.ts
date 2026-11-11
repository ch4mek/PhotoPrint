import { MouseEvent, useMemo } from 'react';

import { useGetTrackCellTooltipMessage } from '@/pages/TimeSheet/components/TimesheetEngineer/hooks/useGetTrackCellTooltipMessage';
import { UseTooltipHandlerProps } from '@/pages/TimeSheet/components/TimesheetEngineer/types';

export const useTooltipHandler = ({
	row,
	column,
	isEngineer,
	setTooltipContent,
	displayTooltips,
}: UseTooltipHandlerProps) => {
	const tooltipMessage = useGetTrackCellTooltipMessage(row, column, isEngineer);
	const shouldDisplayTooltip = displayTooltips && tooltipMessage;

	const handleMouseOver = useMemo(() => {
		if (shouldDisplayTooltip) {
			return (e: MouseEvent<HTMLDivElement>) => {
				setTooltipContent({
					text: tooltipMessage,
					anchorElement: e.currentTarget,
				});
			};
		}
	}, [shouldDisplayTooltip]);

	const handleMouseOut = useMemo(() => {
		if (shouldDisplayTooltip) {
			return () => {
				setTooltipContent(null);
			};
		}
	}, [shouldDisplayTooltip]);

	return {
		handleMouseOver,
		handleMouseOut,
		tooltipMessage,
	};
};
