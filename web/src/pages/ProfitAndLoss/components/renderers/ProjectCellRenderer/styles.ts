import styled from '@emotion/styled';

import { CellWrapper } from '@/components/Grid';
import { ColorCircle } from '@/components/Grid/renderers/cell/cellWithColorCircleRenderer/styles';

export const ProjectCellContent = styled(CellWrapper)({
	display: 'flex',
	alignItems: 'center',
	minWidth: 0,
	width: '100%',
	height: '100%',
	overflow: 'hidden',
});

export const ProjectCellText = styled('span')<{ isChildRow?: boolean }>(
	({ isChildRow, theme }) => ({
		display: 'block',
		minWidth: 0,
		width: '100%',
		overflow: 'hidden',
		textOverflow: 'ellipsis',
		whiteSpace: 'nowrap',
		...(isChildRow
			? theme.typography.body['s-400']
			: theme.typography.body['m-400']),
	})
);

export const ProjectInlineColorCircle = styled(ColorCircle, {
	shouldForwardProp: (prop) => prop !== 'circleSize',
})<{ circleSize?: number }>(({ circleSize, theme }) => ({
	display: 'inline-block',
	marginRight: theme.spaces.xs,
	...(circleSize && {
		width: circleSize,
		height: circleSize,
		minWidth: circleSize,
		minHeight: circleSize,
	}),
}));

export const ProjectNameSeparator = styled.span(({ theme }) => ({
	marginRight: theme.spaces.xs,
}));
