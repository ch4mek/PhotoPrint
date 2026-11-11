import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { CellWrapper } from '../../styles';

import { cellWithTooltipRenderer } from '@/components/Grid';

import { LinkWrapper, RTLWrapper } from './styles';
import { CellWithTaskProps } from './types';

export function cellWithLinkRenderer<T extends CellWithTaskProps>(
	props: RenderCellProps<T, unknown>
) {
	const { row } = props;
	const { showSkeleton, jiraTask } = row;

	const isLink = String(jiraTask).startsWith('http');
	const textWithTooltip = cellWithTooltipRenderer(props);

	return showSkeleton ? (
		<Skeleton animation="wave" width="100%" />
	) : (
		<CellWrapper>
			{isLink ? (
				<LinkWrapper href={jiraTask} target="_blank" rel="noopener noreferrer">
					{textWithTooltip}
				</LinkWrapper>
			) : (
				<RTLWrapper>{textWithTooltip}</RTLWrapper>
			)}
			{textWithTooltip}
		</CellWrapper>
	);
}
