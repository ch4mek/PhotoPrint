import { CollapseFunctionProps, CollapseItemProps } from '@/components/CollapseGroup/renderer/collapseItemRenderer/types';
import { Option } from '@/components/CollapseGroup/types';
export declare function collapseItemRenderer<T extends Option>({ renderCollapseItem, item, }: CollapseItemProps<T>): (({ disabled, expanded, onClick }: CollapseFunctionProps) => import("react").ReactNode) | (({ onClick, disabled, expanded }: CollapseFunctionProps) => import("react/jsx-runtime").JSX.Element);
