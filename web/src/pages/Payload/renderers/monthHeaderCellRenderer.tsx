import { PERIOD } from '@/constants/date';
import { usePayloadContext } from '@/context/PayloadContext';
import { isCurrentYearMonth } from '@/helpers/date';
import { MonthHeaderCell } from '@/pages/Payload/styles';
import { PayloadGridRow } from '@/pages/Payload/types';
import { RenderHeaderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';

export const monthHeaderCellRenderer = ({
	column: { key, hoverIndex },
}: RenderHeaderCellPropsWithHoverIndex<PayloadGridRow, PayloadGridRow>) => {
	const { year } = usePayloadContext();
	const index = PERIOD.findIndex((month) => month === key) + 1;
	const isCurrentMonth = isCurrentYearMonth(index, +year);

	return (
		<MonthHeaderCell
			data-activemonth={isCurrentMonth}
			isCurrentMonth={isCurrentMonth}
			data-hovercolindex={hoverIndex}
		>
			{key}
		</MonthHeaderCell>
	);
};
