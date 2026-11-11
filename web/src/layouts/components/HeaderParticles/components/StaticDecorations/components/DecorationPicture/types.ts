import { ResponsiveValue } from '@/types/breakpoints';

export interface DecorationPictureProps {
	images: ResponsiveValue<string>;
	alt?: string;
	hideBelow?: number;
}
