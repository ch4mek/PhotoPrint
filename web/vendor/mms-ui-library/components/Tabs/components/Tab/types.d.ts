import { TabProps } from '../../types';
export type TabWrapperProps = Pick<TabProps, 'active' | 'fullWidth' | 'disabled' | 'customActiveTabColor' | 'customTabInnerPadding'>;
export type TabContentProps = Pick<TabProps, 'active' | 'fullWidth'>;
