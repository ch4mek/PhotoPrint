import { PropsWithChildren } from 'react';

import type { TabPanelProps } from './types';

export function TabPanel({
	children,
	value,
	index,
}: PropsWithChildren<TabPanelProps>) {
	return value !== index ? null : children;
}
