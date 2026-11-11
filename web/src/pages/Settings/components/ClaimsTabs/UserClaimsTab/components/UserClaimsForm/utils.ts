import { Theme } from '@emotion/react';

import { popUpSizes } from '@/constants/popUp';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import { AvailableUserClaimResponse } from '@/types/settings';

export const getPopUpStylesConfig = (theme: Theme, windowHeight: number) => ({
	container: {
		width: popUpSizes.l,
	},
	content: {
		...getAdaptivePopUpContentStylesConfig({
			theme,
			hasContols: true,
			windowHeight,
		}),
	},
	button: {
		paddingTop: 0,
	},
});

export const getClaimPermissions = (
	permissions: AvailableUserClaimResponse[]
): string[] =>
	permissions.reduce<string[]>((acc, item) => {
		const { name, claims } = item;

		if (name && claims) {
			return [...acc, ...claims.map((claim) => claim.claimType)];
		}

		return acc;
	}, []);
