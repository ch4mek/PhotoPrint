import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { RenderCellProps } from 'react-data-grid';

import { DynamicHeightWrapper } from '@/components/Grid/components/DynamicHeightCellWrapper';
import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { LINK_REPLACE_TITLE } from '@/components/Grid/renderers/cell/cellWithLinksRenderer/constants';
import {
	LinksCellWrapper,
	LinkWrapper,
} from '@/components/Grid/renderers/cell/cellWithLinksRenderer/styles';
import { CellWithLinksProps } from '@/components/Grid/renderers/cell/cellWithLinksRenderer/types';

export function cellWithLinksRenderer<T extends CellWithLinksProps>({
	row,
	column: { idx },
	onRowChange,
}: RenderCellProps<T, unknown>) {
	const { links } = row;

	return (
		<DynamicHeightWrapper row={row} idx={idx} onRowChange={onRowChange}>
			<LinksCellWrapper>
				{links &&
					links.map((link, index) => (
						<Tooltip
							text={link}
							key={index}
							position="left"
							fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
						>
							<LinkWrapper
								href={link}
								target="_blank"
								rel="noopener noreferrer"
							>
								{LINK_REPLACE_TITLE}
							</LinkWrapper>
						</Tooltip>
					))}
			</LinksCellWrapper>
		</DynamicHeightWrapper>
	);
}
