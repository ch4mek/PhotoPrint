import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { memo } from 'react';

import { zIndexes } from '@/constants/zIndexes';

import { DropDownItemWrapper, DropDownItem, DropDownList } from './styles';
import { DropDownProps } from './types';

const OFFSET_Y = 4;

function DropDownInner({ anchorElement, isOpen, items, ref }: DropDownProps) {
	return (
		<DropDownList
			offset={[0, OFFSET_Y]}
			open={isOpen}
			anchorElement={anchorElement}
			position="bottom-end"
			ref={ref}
		>
			{items.map((i) => (
				<DropDownItemWrapper key={i}>
					<Tooltip
						zIndex={zIndexes.gridTooltip}
						text={i}
						position="bottom"
						fallbackPlacements={['bottom']}
						useIfTextCut
					>
						<DropDownItem variant="m-400">{i}</DropDownItem>
					</Tooltip>
				</DropDownItemWrapper>
			))}
		</DropDownList>
	);
}

export const DropDown = memo(DropDownInner);
