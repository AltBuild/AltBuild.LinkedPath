AltBuild.LinkedPath

1) Object に 持たせる機能
　→ ID (object)
　→ Object の 状態 (DObjectRule= Datastore, Copy, Building, Refreshing)
　→ Object の コピー



- Phase 1
  - Path文字列の解析まで
    - LinkedPathParse

- Phase 2
  - 
  - 必要に応じて MemberInfo の作成
  - 必要に応じて PathValue(s) の作成



パスの基本式： m:a=v
              m1[i1].m2[i2]:a=v
              m1[i1].m2(arguments):attributes=values
              m => Members
              a => Attributes (Optional)
              v => Values (Optional) => Values is nested m:a=v

パスの複数式： m:a=v m:a=v m:a=v ..... スペース（or改行）で区切った連結式


パス式：  国立高等学校.学級('xxxx','yyyy').学科[ID=['xxxx-xxxx-xxxx-xxxx']]:readonly,class='text-primary',member='学級.学級名[IsEnable=true]:class='text-danger' 学級.人数 学級.備考'='zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz'
         ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
         | PathMembers                                                : PathAttributes                                                                         = PathValues


         ^^^^^^^^^^^^^^^^                ^^^^
         MemberName                      MemberName

                         ^^^^^^^^^^^^^^^    ^^^^^^^^^^^^^^^^^^^^^^^^^^ ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ 
                         MethodArguments    PropertyArguments          Attributes

