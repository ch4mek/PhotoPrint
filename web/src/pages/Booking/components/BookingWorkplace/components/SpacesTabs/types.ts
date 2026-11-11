import { OfficeWorkspaceType } from '@/types/configuration';

export interface SpacesTabsProps {
	activeTab: number;
	onChange: (newTab: number) => void;
	spaces: Array<OfficeWorkspaceType>;
	hasMeetingRooms: boolean;
	activeTabsId?: (string | number)[];
}
