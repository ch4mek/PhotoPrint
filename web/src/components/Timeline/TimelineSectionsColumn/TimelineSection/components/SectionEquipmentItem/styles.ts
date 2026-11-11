import styled from '@emotion/styled';

import { IconSize } from '@/constants/iconSizes';

export const EquipmentItem = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'flex-end',
	color: theme.palette['graphics-primary-2'],

	'& svg': {
		width: IconSize.S,
		height: IconSize.S,
	},
}));
