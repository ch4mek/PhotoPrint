import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { CellWrapper } from '../../styles';

import { IconSize } from '@/constants/iconSizes';

export const ArrayCellWrapper = styled(CellWrapper)(() => ({
	height: '100%',
	width: '100%',
	maxWidth: '100%',
	display: 'flex',
	alignItems: 'center',
	overflow: 'unset',
}));

export const ArrayDropDownWrapper = styled.div(() => ({
	display: 'flex',
	width: '100%',
}));

export const FirstItem = styled(Typography)(() => ({
	display: 'inline-block',
	textOverflow: 'ellipsis',
	overflow: 'hidden',
	fontSize: 'inherit',
}));

export const ChevronWrapper = styled('div')({
	height: IconSize.MS,
});

export const FirstItemWrapper = styled('div')(
	({ theme }) => css`
		width: 100%;
		cursor: pointer;
		display: flex;
		align-items: center;
		gap: ${theme.spaces.s}px;
		justify-content: space-between;
	`
);
