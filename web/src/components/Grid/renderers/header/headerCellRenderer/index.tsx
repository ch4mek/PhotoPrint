import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { ArrowDownGridIcon } from '@mms/mms-ui-library/icons/base/ArrowDownGridIcon';
import { ArrowUpGridIcon } from '@mms/mms-ui-library/icons/base/ArrowUpGridIcon';
import { useMemo } from 'react';

import { GridCellDefaultProps } from '../../types';

import { InfoTooltipCircle } from '@/components/InfoTooltipCircle';
import { SortDirection } from '@/constants/sort';

import {
	getInfoTooltipCircleStyles,
	HeaderCellBaseContentWrapper,
	HeaderCellContentWrapper,
} from './styles';
import { HeaderCellRendererProps } from './types';

export const HeaderCellRenderer = <
	T extends GridCellDefaultProps,
	TSummaryRow = unknown
>({
	column: { name, sortable },
	sortDirection,
	nameContent,
	tooltipContent,
	isContentAlignedToEnd,
}: HeaderCellRendererProps<T, TSummaryRow>) => {
	const sortIcon =
		sortDirection?.toLowerCase() === SortDirection.ASC ? (
			<ArrowDownGridIcon />
		) : (
			<ArrowUpGridIcon />
		);

	const infoTooltipCircleStylesConfig = useMemo(
		() => getInfoTooltipCircleStyles(),
		[]
	);

	return (
		<HeaderCellContentWrapper
			isHasSortableIcon={sortable}
			isContentAlignedToEnd={isContentAlignedToEnd}
		>
			<HeaderCellBaseContentWrapper isColumnSorted={!!sortDirection}>
				{nameContent ?? <span>{name}</span>}
				{sortDirection && <ActionButton>{sortIcon}</ActionButton>}
			</HeaderCellBaseContentWrapper>

			{tooltipContent && (
				<InfoTooltipCircle
					tooltipContent={tooltipContent}
					stylesConfig={infoTooltipCircleStylesConfig}
				/>
			)}
		</HeaderCellContentWrapper>
	);
};
