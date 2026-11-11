import { MicrofrontendsGridRow } from '../types';

import { MFERegistryResponse } from '@/types/microfrontends';

const BASE_ROW_HEIGHT = 64;
const CHIP_HEIGHT = 28;
const CELL_PADDING = 24;
const EXPECTED_CHIPS_IN_ROW_COEF = 1.5;

export const mapMicrofrontendsResponseToRows = (
	data: MFERegistryResponse,
	handleEdit: (itemId?: string | number) => () => void,
	handleDelete: (itemId?: string | number) => () => void
): MicrofrontendsGridRow[] =>
	data.microfrontends.map<MicrofrontendsGridRow>(({ ...rest }) => {
		const modulesCount = rest.modules?.length || 0;
		const routesCount = rest.routes?.length || 0;
		const claimsCount = rest.claims?.length || 0;

		const maxChipsCount = Math.max(routesCount, claimsCount, modulesCount);
		const chipsRows = Math.ceil(maxChipsCount / EXPECTED_CHIPS_IN_ROW_COEF);

		const itemRowHeight =
			chipsRows > 1
				? BASE_ROW_HEIGHT + (chipsRows - 1) * CHIP_HEIGHT
				: BASE_ROW_HEIGHT;

		return {
			...rest,
			itemRowHeight: Math.max(BASE_ROW_HEIGHT, itemRowHeight + CELL_PADDING),
			handleEdit,
			handleDelete,
		};
	});
