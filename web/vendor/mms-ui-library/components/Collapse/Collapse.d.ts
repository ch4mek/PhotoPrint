import { Option } from '@/components/CollapseGroup/types';
import { CollapseProps } from './types';
export declare function Collapse<T extends Option>({ children, maxWidth, easing, timeout, unMountOnExit, disabled, fullwidth, renderHeader, onTransitionEnd, onClick, collapseGroupWrapper, hasTopLevelStyles, contentStyles, item, toggleCollapse, ...restProps }: CollapseProps<T>): import("react/jsx-runtime").JSX.Element;
