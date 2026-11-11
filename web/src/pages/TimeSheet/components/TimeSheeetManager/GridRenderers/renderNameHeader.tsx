import { NameHeaderWrapper } from '../styles';
import { TimeSheetManagerRow } from '../types';

import { RenderHeaderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';

export const renderNameHeader = ({
	column: { name, hoverIndex },
}: RenderHeaderCellPropsWithHoverIndex<TimeSheetManagerRow, unknown>) => (
	<NameHeaderWrapper data-hovercolindex={hoverIndex}>
		<span>{name}</span>
	</NameHeaderWrapper>
);
