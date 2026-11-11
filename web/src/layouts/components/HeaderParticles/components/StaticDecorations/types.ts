import { CSSObject } from '@emotion/react';

import { ResponsiveValue } from '@/types/breakpoints';

export type ZonePosition = 'left' | 'center' | 'right';
export type ZoneContentAlign = 'flex-start' | 'center' | 'flex-end';
type DecorationPlacement = 'bottom' | 'top';

interface DecorationItem {
	id: string;
	images: ResponsiveValue<string>;
	alt?: string;
	hideBelow?: number;
}

interface DecorationZoneConfig {
	items: DecorationItem[];
	position: ZonePosition;
	contentAlign?: ZoneContentAlign;
	gap?: ResponsiveValue<number>;
	hideBelow?: number;
	styles?: ResponsiveValue<CSSObject>;
}

export interface StaticDecorationsConfig {
	zones: DecorationZoneConfig[];
	placement?: DecorationPlacement;
	zonesGap?: ResponsiveValue<number>;
}
