import { breakpoints } from '@mms/mms-ui-library/theme';

export type DeviceBreakpointKey = 'desktop' | 'tablet' | 'mobile';
export type ThemeBreakpointKey = keyof typeof breakpoints;

type ResponsiveBreakpointKey =
	| DeviceBreakpointKey
	| ThemeBreakpointKey
	| number;

export type ResponsiveValue<T> = Partial<Record<ResponsiveBreakpointKey, T>>;
