import { Option } from '@/pages/Wiki/types';
import { SegmentTypes } from '@/types/wiki';

export interface UseKnowledgeGroupFormProps {
	segmentType: SegmentTypes | null;
}

export interface GroupsItems {
	items: Option[];
}

export interface CollapseWrapperProps<T> {
	item: T;
	hasTopLevelStyles: boolean;
}
