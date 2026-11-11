import { MinskSpacesInternalCodes } from '@/types/booking';

import { WorkSpaceConfigurationGenerator } from './types';
import {
	generateMinskSpaceA,
	generateMinskSpaceB,
	generateMinskSpaceC,
	generateMinskSpaceD,
	generateMinskSpaceE,
	generateMinskSpaceF,
	generateWarsawSpace,
} from './utils';

export const bookingGridAreas = {
	titleArea: 'title',
	legendArea: 'legend',
	workplaceArea: 'workplace',
};

export const WARSAW_SPACE: WorkSpaceConfigurationGenerator =
	generateWarsawSpace;

type LocationSpaces = {
	[key in MinskSpacesInternalCodes]: WorkSpaceConfigurationGenerator;
};

export const MINSK_SPACES: LocationSpaces = {
	[MinskSpacesInternalCodes.SpaceA]: generateMinskSpaceA,
	[MinskSpacesInternalCodes.SpaceB]: generateMinskSpaceB,
	[MinskSpacesInternalCodes.SpaceC]: generateMinskSpaceC,
	[MinskSpacesInternalCodes.SpaceD]: generateMinskSpaceD,
	[MinskSpacesInternalCodes.SpaceE]: generateMinskSpaceE,
	[MinskSpacesInternalCodes.SpaceF]: generateMinskSpaceF,
};
