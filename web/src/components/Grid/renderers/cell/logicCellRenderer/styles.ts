import styled from '@emotion/styled';

import { CellWrapper } from '../../styles';

import { SMALL_ICON_SIZE } from '@/constants/commonStyles';

export const LogicCellWrapper = styled(CellWrapper)(({ theme }) => ({
	height: '100%',
	display: 'flex',
	alignItems: 'center',
	color: theme.palette['graphics-primary-2'],
	svg: {
		width: SMALL_ICON_SIZE,
		height: SMALL_ICON_SIZE,
	},
}));
