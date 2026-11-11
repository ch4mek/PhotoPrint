import { Theme } from '@emotion/react';

import { POPUP_HEADER_HEIGHT } from '@/constants/popUp';
import { formatDateString } from '@/helpers/dateToISOString';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import { UserProjectLookup } from '@/types/userProjectLookup';

import { ProjectTableRow } from './types';

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	container: {
		width: '90vw',
		height: '100%',

		'& > div': {
			height: `calc(100% - ${POPUP_HEADER_HEIGHT}px)`,
		},
	},
	content: {
		...getAdaptivePopUpContentStylesConfig({ theme, windowHeight }),
		padding: `${theme.spaces.m}px ${theme.spaces.l}px ${theme.spaces.l}px ${theme.spaces.l}px`,
	},
});

export const getRowKey = (row: ProjectTableRow) => row.id;

export const convertUserProjectsToRows = (
	projects: UserProjectLookup[]
): ProjectTableRow[] =>
	projects.map(
		({ userOnProjectFrom, userOnProjectTo, isActive, ...resProps }) => ({
			...resProps,
			isActive,
			userOnProjectFrom: formatDateString(userOnProjectFrom, ''),
			userOnProjectTo: formatDateString(userOnProjectTo, ''),
			disabled: !isActive,
		})
	);
