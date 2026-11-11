import { PropsWithChildren } from 'react';

import { Option } from '@/pages/Wiki/types';

export interface CollapseItemWrapperProps extends PropsWithChildren {
	hasTopLevelStyles: boolean;
	item: Option;
	isDragMode: boolean;
}
