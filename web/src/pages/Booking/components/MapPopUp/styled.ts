import styled from '@emotion/styled';

import { POPUP_TABS_HEIGHT } from '@/constants/popUp';

import {
	OFFICE_MAP_HEIGHT_PX,
	OFFICE_MAP_WIDTH_PX,
	POPUP_WIDTH_VW,
} from './constants';

export const MapImageContainer = styled('div')(
	({ theme }) =>
		({
			padding: theme.spaces.l,
			maxWidth: '100%',
			maxHeight: '100%',
			height: `calc((${POPUP_WIDTH_VW}vw - ${2 * theme.spaces.l}px) * ${
				OFFICE_MAP_HEIGHT_PX / OFFICE_MAP_WIDTH_PX
			} + ${POPUP_TABS_HEIGHT}px)`,
			display: 'flex',
			justifyContent: 'center',

			'& > img': {
				objectFit: 'contain',
				maxWidth: '100%',
				maxHeight: '100%',
			},
		} as const)
);
