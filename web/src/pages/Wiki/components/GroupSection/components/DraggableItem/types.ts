import { PropsWithChildren } from 'react';

import { Option } from '@/pages/Wiki/types';

export interface DraggableItemProps extends PropsWithChildren {
	isDragMode: boolean;
	item: Option;
}
