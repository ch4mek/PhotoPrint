import { Section, TimelineProps } from '../types';

export interface TimelineSectionsColumnProps
	extends Pick<TimelineProps, 'selectedSectionId' | 'onSectionSelection'> {
	sections: Section[];
}
